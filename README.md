# MailCheck.AggregateReport
The Mail Check AggregateReport Microservice is responsible for processing DMARC reports fed to it by an S3 bucket. The service also "aggregates" this data in useful formats which are then fed to the front-end to be displayed.
You should have a database connection established with the AggregateReport DB when running any project which contains a DAO folder. If running any of the Intelligence.Api projects, you will need to establish a connection to the AggregateReport Postgres DB.
## Environment Variables needed for running:
In general, when running any project within MailCheck.AggregateReport you should have the following environment variables set up:
|Variable  | Value |
|--|--|
| AuthorisationServiceEndpoint | url of authorisation endpoint| 
| SnsTopicArn | arn of topic to publish messages  |
| ConnectionString | database connection string | 
| DevMode | boolean to toggle CORS and run on localhost | 
| AWS_REGION | aws datacentre region  |
| AWS_ACCESS_KEY_ID |aws access key  |
| AWS_SECRET_ACCESS_KEY |aws secret access key  |



### AggregateReport.Parser specific environment variables (as well as the above)
|Variable  | Value |
|--|--|
| DkimSelectorsTopicArn |	arn of topic for publishing DkimSelectorsSeen messages   | 
| TimeoutSqs |	SQS timeout in seconds|
| TimeoutS3 |	S3 timeout in seconds | 
| MaxS3ObjectSizeKilobytes |	Limit in kb of S3 objects|