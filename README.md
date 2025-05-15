## Brønnøysundregistrene CSV parser

This project is split into two parts, a .NET 9 rest api, and a React frontend.
The frontend accepts user input and displays the processed data.
The backend parses the CSV, and processes all entries via the entity registry from Brønnøysundregistrene, where it is enriched and used to generate statistics for the entire request.

The backend is split into four services: CsvParser, Company service, Statistics service and Brønnøysundservice.
* The Csv parser parses raw CSV data into a strutured format
* The Brønnøysundservice maps entity ids into real Entities, before mapping this data into the generalized company dto.
* The Company service orchestrates the conversion from IDs to company entities, and handles any malformed company results.
* The Statistic service uses the generated data from the Company service to do some simple calculations for the whole dataset.

## Improvements
* A bit too much orchestration logic ended up in the controller. Ideally this should be extracted, and the controller should only be responsible for handling http/web related issues.
* Should have included tests for the Statistic service as well.
* The styling on the frontend should have been cleaned up. Too much time was being spent trying to make it look right before discarding most of it.
  
## Running the project
### Docker compose
```
docker compose up
```

### Manually
Frontend

`cd frontend && npm install && npm run dev`

Backend

`cd BrønnApi && dotnet run`

Frontend can be accessed at: `http://localhost:5173/`
