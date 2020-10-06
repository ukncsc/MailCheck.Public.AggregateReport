GRANT SELECT, INSERT, UPDATE ON `domain_date_provider` TO '{env}-dat-dom-pro' IDENTIFIED BY '{password}';
GRANT SELECT, INSERT, UPDATE ON `domain_date_provider_store` TO '{env}-dat-dom-pro' IDENTIFIED BY '{password}';
GRANT SELECT ON `domain_date_provider` TO '{env}-agg-api' IDENTIFIED BY '{password}';
