## Experimentation API

The API provides a RESTful interface over HTTP that makes it easy to dynamically toggle on/off your production code when it starts to fail or is simply exhibiting unexpected behaviour and needs to be taken offline for further analysis. Allowing you the ability to rest easy knowing that you don't have to issue an emergency hotfix. With the API you can make a `HTTPDELETE` request to remove a Feature from the DB and all future requests made to the API will resolve to a `404 Not Found`. Then your clients will stop executing the faulty code and carry on running. 

Ofcourse this is just one way to implement a 'Feature-Switching' service, semantically above as described is what makes sense to me but you can integrate and extend the API and consume it however you want.


### Feature Switching

[Martin Fowlers Blog:](https://martinfowler.com/articles/feature-toggles.html)
>Feature toggles are a powerful technique, allowing teams to modify system behavior without changing code. They fall into >various usage categories, and it's important to take that categorization into account when implementing and managing toggles. >Toggles introduce complexity. We can keep that complexity in check by using smart toggle implementation practices and >appropriate tools to manage our toggle configuration, but we should also aim to constrain the number of toggles in our system. 

```markdown
Syntax highlighted code block


