GRANT SELECT, INSERT, UPDATE ON `domain_date_provider_subdomain` TO '{env}-dat-dom-pro-sub-rol' IDENTIFIED BY '{password}';
GRANT SELECT, INSERT, UPDATE ON `domain_date_provider_subdomain_store` TO '{env}-dat-dom-pro-sub-rol' IDENTIFIED BY '{password}';
GRANT SELECT ON `domain_date_provider_subdomain` TO '{env}-agg-api' IDENTIFIED BY '{password}';
