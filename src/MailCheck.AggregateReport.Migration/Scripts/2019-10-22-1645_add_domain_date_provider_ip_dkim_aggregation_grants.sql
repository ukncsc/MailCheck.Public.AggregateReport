GRANT SELECT, INSERT, UPDATE ON `domain_date_provider_ip_dkim` TO '{env}-dat-dom-pro-ip-dkim' IDENTIFIED BY '{password}';
GRANT SELECT, INSERT, UPDATE ON `domain_date_provider_ip_dkim_store` TO '{env}-dat-dom-pro-ip-dkim' IDENTIFIED BY '{password}';
GRANT SELECT ON `domain_date_provider_ip_dkim` TO '{env}-agg-api' IDENTIFIED BY '{password}';
