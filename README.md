# Checkout

### Folder Structure
##### Checkout.PaymentGateway
##### Checkout.UnitTest

### Features Available
- IP origin validator : Each merchant is expected to supply a list of IPs that will be used in communicate with the Payment Gateway. This can be configured in the appsetting files based on environment
- Global Exception handler : This captures all exceptions without the use of try catch in the code
- AesAlgorithm : Used for Encryption & Decryption

#### Endpoint
Make Payment
Get Transaction
  - With Merchant Id
  - With Transaction ID or
  - With Both
Get Currency

#### Library Used
-- Automapper (Object transformation) <br />
-- Fluent Validation (Model Validation) <br />
-- Swagger (API test in dev environment) <br />
-- Polly (Retry) <br />
-- Seriolog (Logging) <br />
-- Shoudly (Test Assertions) <br />
-- NSubstitute (Mock) <br />

#### Implementation
---- Controller <br />
--------- Service <br />
-------------- Repository <br />

No actual db was used in this implementation, reason why I declare the Table Model repository as Singleton so as to keep the state of application

#### Screenshots
Make Payment - https://127.0.0.1:7295/api/payment/makepayment
<img width="1049" alt="image" src="https://github.com/ifegade/Checkout/assets/3215021/aa9547f6-b4bf-4586-8af8-2b67819419b6">
Get Transaction(s) - https://127.0.0.1:7295/api/payment/getcurrency
<img width="1049" alt="image" src="https://github.com/ifegade/Checkout/assets/3215021/a89ac7ac-1c1e-4e57-abc4-05bef22ca719">
Get Currency - https://127.0.0.1:7295/api/payment/getcurrency
<img width="1049" alt="image" src="https://github.com/ifegade/Checkout/assets/3215021/9b3dcf66-411b-45e5-911b-8abc22e5a2bd">
Unit Test Coverage
<img width="554" alt="image" src="https://github.com/ifegade/Checkout/assets/3215021/0c36dc70-e0f7-4885-8740-829dbf192359">




