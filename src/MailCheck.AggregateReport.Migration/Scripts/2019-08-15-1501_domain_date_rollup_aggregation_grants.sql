GRANT SELECT, INSERT, UPDATE ON `domain_date_rollup` TO '{env}-dat-dom-ru' IDENTIFIED BY '{password}';
GRANT SELECT, INSERT, UPDATE ON `domain_date_rollup_store` TO '{env}-dat-dom-ru' IDENTIFIED BY '{password}';
GRANT SELECT ON `domain_date_rollup` TO '{env}-agg-api' IDENTIFIED BY '{password}';
