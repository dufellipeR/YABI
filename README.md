# Yabi

## Description
**Yabi** stands for **Yet Another Bitcoin Index**. It is an **ASP.NET Core** application that provides a compact of Bitcoin Market Indexes into one index with scale of 1 to 21, it returns the generated Yabi index.

### Features
- The Yabi Index is automatically generated *once a day* ( you can change this on the code for test purposes) by a cronjob.
- Indexes in V1 include :
1. Fear and Greed
2. Mayer Multiple (2.4)
- All indexes represent the same weight when generating Yabi Index.
  

## Prerequisites
- .NET 8.0 SDK

## Installation
1. Clone the repository.
2. Navigate to the project directory.
3. Run the following command:
    ```sh
    dotnet run
    ```
4. Open your web browser and navigate to Swagger Docs  `http://localhost:5259/swagger/index.html`.

## Usage
1. Open your web browser and navigate to `http://localhost:5259`.
2. The application will display a json with the current Yabi Index and the datetime which it was generated (if there's any).

## Future Work
- Integrate Cache
- Add more indexes
- Safety measures
- Create a client
- Deploy

## License
This project is licensed under the MIT License - see the LICENSE file for details.
