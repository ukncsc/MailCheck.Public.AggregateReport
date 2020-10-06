GRANT SELECT, INSERT, UPDATE ON `domain_date_provider_rollup` TO '{env}-dat-dom-pro-rol' IDENTIFIED BY '{password}';
GRANT SELECT, INSERT, UPDATE ON `domain_date_provider_rollup_store` TO '{env}-dat-dom-pro-rol' IDENTIFIED BY '{password}';
GRANT SELECT ON `domain_date_provider_rollup` TO '{env}-agg-api' IDENTIFIED BY '{password}';
