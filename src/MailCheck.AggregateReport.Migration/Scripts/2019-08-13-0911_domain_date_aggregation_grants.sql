GRANT SELECT, INSERT, UPDATE ON `domain_date` TO '{env}-dat-dom' IDENTIFIED BY '{password}';
GRANT SELECT, INSERT, UPDATE ON `domain_date_store` TO '{env}-dat-dom' IDENTIFIED BY '{password}';
GRANT SELECT ON `domain_date` TO '{env}-agg-api' IDENTIFIED BY '{password}';