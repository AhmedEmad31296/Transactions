# Transactions

1- Gateway: Web API Restful web service accept json request encrypted using HTTP post and return JSON response with result for this transaction
2- Client Application: Angular web application to accept transaction data in form and send it to web service as HTTP post and display result

Details

 
•	Client Application support user authentication to validate user information.
•	Auth. user enter transaction data to process and application send data to gateway to get approval code for transaction. 
•	all transaction data passed between web application and service encrypted.
•	Web API Service have method to generate encryption key for each transaction and pass it to client application to encrypt data before sending transaction data
•	When Web API service receive encrypted transaction and decrypted it and generate Approval Code as 6 digits and return transaction response.
•	Transaction data passed in request to service in minimized data length < 60 bytes 


