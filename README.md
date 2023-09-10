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
Automapper (Object transformation)
Fluent Validation (Model Validation)
Swagger (API test in dev environment)
Polly (Retry)
Seriolog (Logging)
Shoudly (Test Assertions)
NSubstitute (Mock)

#### Implementation

