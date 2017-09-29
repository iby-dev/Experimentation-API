## Experimentation API

The API provides a RESTful interface over HTTP that makes it easy to dynamically toggle on/off your production code when it starts to fail or is simply exhibiting unexpected behaviour and needs to be taken offline for further analysis. Allowing you the ability to rest easy knowing that you don't have to issue an emergency hotfix. With the API you can make a `HTTPDELETE` request to remove a Feature from the DB and all future requests made to the API will resolve to a `404 Not Found`. Then your clients will stop executing the faulty code and carry on running. 

Ofcourse this is just one way to implement a 'Feature-Switching' service, semantically above as described is what makes sense to me but you can integrate and extend the API and consume it however you want.


### Feature Switching

[Martin Fowlers Blog:](https://martinfowler.com/articles/feature-toggles.html)
>Feature toggles are a powerful technique, allowing teams to modify system behavior without changing code. They fall into >various usage categories, and it's important to take that categorization into account when implementing and managing toggles. >Toggles introduce complexity. We can keep that complexity in check by using smart toggle implementation practices and >appropriate tools to manage our toggle configuration, but we should also aim to constrain the number of toggles in our system. 

## Tooling

The API was built using the following libraries:
- Asp.net Core 2.0
- .net Core 2.0
- MongoDB
- Swagger
- Autofac
- Serilog
- Polly
- NetStandard Library

## Coming Soon?

To make it easier to work with the API i will be uploading a simple nuget package that you can be downloaded and added to your internal package repositories whether that is on NEXUS (Sonatype) or TeamServices. The package will wraps access how the API works and the benefits of using the two together. 

## Potential Upgrades

1. Caching? - I want to enable client side caching of requests so inside the nuget package that makes the request to the API and then interprets the response from the API. 
2. Enable Feature Switching by Buckets - This is the next major upgrade I can think - essentially allow entity ids to be associated to a switch to be stored in buckets. i.e User Ids, or Application Ids, Employee Ids or CustomerIds.... whatever you want? 

## The Simple Case For MongoDB
The api is designed to work with MongoDB - you can however fork the code and choose to work with EFCore if you want but for now MongoDB seemed like a better fit to me. The api does not store structured data or relational data so i thought it might be a good idea to research NoSQL Dbs. With the idea being of storing a simple collection of data that grows vertically in size but not horizontally.

Setup Help: [Stack Overflow Setup Help](https://stackoverflow.com/questions/2438055/how-to-run-mongodb-as-windows-service)
