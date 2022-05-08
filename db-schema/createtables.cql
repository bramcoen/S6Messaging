CREATE KEYSPACE IF NOT EXISTS messages WITH replication = 
{'class':'SimpleStrategy','replication_factor':'1'};

CREATE TABLE IF NOT EXISTS messages.model
(
  timestamp timestamp,
  id varchar,
  PRIMARY KEY (id, timestamp)
) WITH CLUSTERING ORDER BY (timestamp DESC);