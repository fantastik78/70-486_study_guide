> This chapter should cover:
> - [Design a hybrid application]()
> - [Plan for session management in a distributed environment]()
> - [Plan web farms]()
> - [Run Microsoft Azure services on-premises with Azure Pack]()
> - [Enable deferred processing through Azure features including queues, scheduled and on-demand jobs, Azure Functions, and Azure Web Jobs]()

## Design a hybrid application
## Plan for session management in a distributed environment
## Plan web farms

A web farm is a group of multiple servers, usually a load balancer reroute to available servers.

Pros of using a web farm:
 * Reliability/availability
 * Capacity/performance
 * Scalability
   *  Azure App Service can automatically add or remove nodes at the request of the system administrator or automatically without human intervention.
 * Maintainability

In a web farm, app needs to configure a Data Protection and Caching strategie.

### Data Protection

### Caching

In a web farm the strategie about caching is to use a ditributed cache shared by multiple app servers.
it's:
 * Coherent
 * Survives server restarts and app deployments
 * Does't use local memory

## Run Microsoft Azure services on-premises with Azure Pack
## Enable deferred processing through Azure features including queues, scheduled and on-demand jobs, Azure Functions, and Azure Web Jobs