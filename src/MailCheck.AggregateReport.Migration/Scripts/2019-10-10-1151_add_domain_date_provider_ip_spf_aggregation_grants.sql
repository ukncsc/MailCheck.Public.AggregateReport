GRANT SELECT, INSERT, UPDATE ON `domain_date_provider_ip_spf` TO '{env}-dat-dom-pro-ip-spf-rol' IDENTIFIED BY '{password}';
GRANT SELECT, INSERT, UPDATE ON `domain_date_provider_ip_spf_store` TO '{env}-dat-dom-pro-ip-spf-rol' IDENTIFIED BY '{password}';
GRANT SELECT ON `domain_date_provider_ip_spf` TO '{env}-agg-api' IDENTIFIED BY '{password}';
