import functions = require('firebase-functions');
import admin = require('firebase-admin');
admin.initializeApp();

// Sends a notifications to all devices when a new message is sent.
exports.sendNotifications = functions.database.ref('chats/{messageId}').onCreate(
    async (snapshot, context) => {
      // Notification details.
      const messageContent = snapshot.val();
      const text: string = messageContent.Message;
      const payload = {
        notification: {
          title: 'Private Messenger: New message',
          body: text ? (text.length <= 100 ? text : text.substring(0, 97) + '...') : ''
        }
      };
  
      // Get the list of device tokens.
      const tokens: any[] = [];
      await admin.database().ref('tokens').once('value', async function(snap: admin.database.DataSnapshot){ 
        if (snap.hasChildren()){
            snap.forEach((childSnapshot: admin.database.DataSnapshot) => { 
                const value = childSnapshot.val();
                if (messageContent.SenderToken !== value.Token){
                    tokens.push(value);
                }
            });

            if (tokens.length > 0){
                const response = await admin.messaging().sendToDevice(tokens.map(x => x.Token), payload);
                await cleanupTokens(response, tokens);
                console.debug('Notifications sent successfully.');
            } else {
                console.debug('Found no devices to notify.');
            }
        } else{
            console.debug('No tokens present in database.');
        }
      });
    });

// Cleans up the tokens that are no longer valid.
function cleanupTokens(response: admin.messaging.MessagingDevicesResponse, tokens: any[]) {
    const promises: Promise<void>[] = [];
    response.results.forEach((result: admin.messaging.MessagingDeviceResult, index: number) => {
      const error = result.error;
    
      if (error) {
        const errorToken = tokens[index];
        console.error('Failure sending notification to', errorToken, error);
        if (error.code === 'messaging/invalid-registration-token' || error.code === 'messaging/registration-token-not-registered'){
            promises.push(admin.database().ref(`tokens/${errorToken.toString()}`).remove());
        }
      }
    });
    
    return Promise.all(promises);
}