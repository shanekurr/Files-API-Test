# WebAPI

## Details
This project is a basic Web API using .NET 6, SqlLite and EntityFramework. The purpose of the API is to handle files with versioning.

## Instructions
Clone the repo to your environment, and open the WebAPI.sln file.
Run the solution, make sure FilesAPI is selected as target project.
Swagger should open with all endpoints available.

## Endpoints
- GET : Files/
  * List endpoint that returns a list of all files in the database.
- GET : Files/{fileId}
  * Returns single file record designated by File ID.
- POST : Files/
  * Accepts file passed in, stores in server's storage, and saves record in database. Returns new file record.
- PUT : Files/{fileId}
  * Updates existing file record and creates new version of file in server's storage. Returns updated file record.
- DELETE : Files/{fileId}
  * Deletes file record, and all file versions that have been saved to server storage.

## TODO
- Add proper exception handling
- Make storage location configurable
- Add download endpoint
- Create proper test harness with separate db and file storage
