GRANT SELECT, INSERT, UPDATE ON `domain_date_provider_ip` TO '{env}-dat-dom-pro-ip-rol' IDENTIFIED BY '{password}';
GRANT SELECT, INSERT, UPDATE ON `domain_date_provider_ip_store` TO '{env}-dat-dom-pro-ip-rol' IDENTIFIED BY '{password}';
GRANT SELECT ON `domain_date_provider_ip` TO '{env}-agg-api' IDENTIFIED BY '{password}';
