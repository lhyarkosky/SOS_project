-- This folder will house the queries for db opperations that will be accessible via an API to the user. This will be primarily used for saving game, loading old game, etc.
-- I just included this because I assume there will be more to the project as the semester goes on than just the SOS game.
/* Given the structure for the project that I've chosen, the controller will call by a a cs file in the repository layer which will call the sql file,
   and if the query is data-retrieving then I will create a model that will hold this data, then the repository will return that model to the controller(or possibly
   another intermediate layer like service) and finally the controller will return the data to the client.
*/