# Project2AzureFunc
There are 2 endpoints:

1. A GET to create new accounts, "api/Account?accountID={ACC_ID_INTEGER}". All new accounts have balance 0.
2. A POST for transactions, "api/Transaction" with a JSON body of TransactionRequest.

The Database is a pre-created MDF file consisting of 2 tables. It is marked as an "embedded resource", so a new copy of it is saved in /debug and /release on each run. I.e., an empty database is provided on each run.

If one wants it to persist, go to /Data/Database.mdf and in Properties, set its type "Resource" (instead of Embedded Resource). That would mean it won't be copied to runtime anymore so do provide a direct Path to it in /Data/DataAccessHelper.cs

NOTE: The AccountID is an integer (per request document), so do limit the accountID within integer range. Since TransactionID had no specification, a GUID is used for it instead
