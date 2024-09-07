# Introduction
This project contains a generic Web API (PreferencesAPI) which exposes a PostgreSQL database, allowing for the creation, update, deletion or fetching of a "Preference". 

The emphasis of this project was to use the microservices architecture, in that the PreferencesAPI can be any API, but consumes two separate microservices (and repositories): the [RedisRateLimitAPI](https://github.com/BradleyCSO/RedisRateLimitAPI) and [RedisCacheAPI](https://github.com/BradleyCSO/RedisCacheAPI).

Both of these microservices (minimal APIs) each have a single responsibility:

### RedisCacheAPI
The RedisCacheAPI is responsible for caching the response body of any API that consumes it.

### RedisRateLimitAPI
The RedisRateLimitAPI is responsible for rate limiting an endpoint, provided it is an endpoint that it is an endpoint that has been configured to be rate limited, determined by the [JSON](RedisRateLimitAPI).

The project is built with .NET 8.0 and uses Docker to containerise the `PreferencesAPI`, `RedisCacheAPI`, `RedisRateLimitAPI` and their required dependencies. 

# API Specification

## Main Functionality

### 1. **Create Preference**
- **Test Case:** Upon creating a Preference via the `CreatePreference` endpoint, the preference should be persisted in the PostgreSQL database
- **Error Handling:** If a Preference already exists with the provided details, the API should:
  - Return a **409 Conflict** status
  - Throw a `PreferencesConflictException`

### 2. **Update Preference**
- **Test Case:** If a Preference record exists, it can be updated via the `UpdatePreference` endpoint, and a **204 No Content** status should be returned
- **Error Handling:**
  - If the record does not exist, return a **304 Not Modified** status

### 3. **Delete Preference**
- **Test Case:** A Preference record can be deleted using the `DeletePreference` endpoint, and return a **204 No Content** status
- **Error Handling:**
  - If the record does not exist, return a **304 Not Modified** status

### 4. **Fetch Preference**
- **Initial Fetch:** The first time a Preference is fetched via the `GetPreferences` endpoint:
  - The service should call <code>DatabaseService</code> to retrieve the record
  - The record should be cached in Redis with the TTL specified in `RedisCacheAPI`
  - The fetched Preference should be returned as part of the API response

- **Subsequent Fetches:** 
  - After caching, subsequent requests should not access `DatabaseService`
  - The response should be served directly from the cache until TTL specified or manual key eviction

### 5. **Rate Limiting**
- **Test Case:** If the rate of fetching Preference records exceeds the limit within the window specified by `RedisRateLimit`, the API should:
  - Return a **429 Too Many Requests** status
# Getting Started
### Prerequisites
Before you start, please make sure you have the following installed:
- [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0) - For building and running the application
- [Docker](https://www.docker.com/get-started) - For containerising the application
- [Git](https://git-scm.com/) - for pulling `RedisRateLimit` and `RedisCacheAPI` [submodules](https://git-scm.com/book/en/v2/Git-Tools-Submodules)

### Running the Project
1. **Pull Git Submodules**
   ```bash
   git submodule update
2. **Build and Start the Services**

   Navigate to the project directory and run the following command:

   ```bash
   docker-compose up --build
3. **Alternatively**

   If you'd like to get the project's dependencies only and run the project locally, you can use the following command:

   ```bash
   docker-compose pull
4. **pgAdmin**

    One image is pgAdmin which can be used to manage the database. The login credentials for this are defined in the <code>docker-compose.yml</code>file

# Next steps?
The focus of this project was an exercise on the microservices architecture using containerisation via Docker. 
An important next step would be increasing resiliency (thinking about what happens when one of the microservices are down -- retries, circuit breakers) via a library such as [Polly](https://www.pollydocs.org/) with [resilience pipelines](https://www.pollydocs.org/advanced/testing.html) for testing.

It is also worth thinking about projects I've worked on the past retrospectively going forward -- how useful functionality can be decoupled and easily added onto projects like these, such as the [Inventory API project](https://github.com/BradleyCSO/inventory-api), which includes JWT authentication that is currently tightly coupled to the API itself. This can be separated into its own API and made to be agnostic of any project or language.
