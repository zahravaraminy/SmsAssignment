Provide the Request Body: 
   
for Post http://localhost:5115/api/sms/can-send

The can-send method expects an SmsRequest object in the body. An example JSON payload might look like this:

{
    "PhoneNumber": "1234567890",
    "AccountId": "Account1"
}

