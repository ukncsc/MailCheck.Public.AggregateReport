GRANT SELECT INTO S3 ON *.* TO '{env}-agg-par' IDENTIFIED BY '{password}';
GRANT INSERT, UPDATE ON `dkim_auth_result` TO '{env}-agg-par' IDENTIFIED BY '{password}';
GRANT INSERT, UPDATE ON `record` TO '{env}-agg-par' IDENTIFIED BY '{password}';
GRANT INSERT, UPDATE ON `spf_auth_result` TO '{env}-agg-par' IDENTIFIED BY '{password}';
GRANT INSERT, UPDATE ON `policy_override_reason` TO '{env}-agg-par' IDENTIFIED BY '{password}';
GRANT SELECT (id), INSERT, UPDATE ON `aggregate_report` TO '{env}-agg-par' IDENTIFIED BY '{password}';

GRANT SELECT ON `domain_date` TO '{env}-agg-api' IDENTIFIED BY '{password}';
GRANT SELECT ON `domain_date_rollup` TO '{env}-agg-api' IDENTIFIED BY '{password}';
GRANT SELECT ON `domain_date_provider` TO '{env}-agg-api' IDENTIFIED BY '{password}';
GRANT SELECT ON `domain_date_provider_rollup` TO '{env}-agg-api' IDENTIFIED BY '{password}';
GRANT SELECT ON `domain_date_provider_subdomain` TO '{env}-agg-api' IDENTIFIED BY '{password}';
GRANT SELECT ON `domain_date_provider_ip` TO '{env}-agg-api' IDENTIFIED BY '{password}';
GRANT SELECT ON `domain_date_provider_ip_spf` TO '{env}-agg-api' IDENTIFIED BY '{password}';
GRANT SELECT ON `domain_date_provider_ip_dkim` TO '{env}-agg-api' IDENTIFIED BY '{password}';
GRANT SELECT ON `eslr` TO '{env}-agg-api' IDENTIFIED BY '{password}';

GRANT SELECT, INSERT, UPDATE ON `domain_date` TO '{env}-dat-dom' IDENTIFIED BY '{password}';
GRANT SELECT, INSERT, UPDATE ON `domain_date_store` TO '{env}-dat-dom' IDENTIFIED BY '{password}';

GRANT SELECT, INSERT, UPDATE ON `domain_date_rollup` TO '{env}-dat-dom-ru' IDENTIFIED BY '{password}';
GRANT SELECT, INSERT, UPDATE ON `domain_date_rollup_store` TO '{env}-dat-dom-ru' IDENTIFIED BY '{password}';


GRANT SELECT, INSERT, UPDATE ON `domain_date_provider` TO '{env}-dat-dom-pro' IDENTIFIED BY '{password}';
GRANT SELECT, INSERT, UPDATE ON `domain_date_provider_store` TO '{env}-dat-dom-pro' IDENTIFIED BY '{password}';


GRANT SELECT, INSERT, UPDATE ON `domain_date_provider_rollup` TO '{env}-dat-dom-pro-rol' IDENTIFIED BY '{password}';
GRANT SELECT, INSERT, UPDATE ON `domain_date_provider_rollup_store` TO '{env}-dat-dom-pro-rol' IDENTIFIED BY '{password}';


GRANT SELECT, INSERT, UPDATE ON `domain_date_provider_subdomain` TO '{env}-dat-dom-pro-sub-rol' IDENTIFIED BY '{password}';
GRANT SELECT, INSERT, UPDATE ON `domain_date_provider_subdomain_store` TO '{env}-dat-dom-pro-sub-rol' IDENTIFIED BY '{password}';


GRANT SELECT ON `spf_auth_result` TO '{env}_domstat' IDENTIFIED BY '{password}';
GRANT SELECT ON `dkim_auth_result` TO '{env}_domstat' IDENTIFIED BY '{password}';
GRANT SELECT ON `policy_override_reason` TO '{env}_domstat' IDENTIFIED BY '{password}';

GRANT SELECT, INSERT, UPDATE ON `domain_date_provider_ip` TO '{env}-dat-dom-pro-ip-rol' IDENTIFIED BY '{password}';
GRANT SELECT, INSERT, UPDATE ON `domain_date_provider_ip_store` TO '{env}-dat-dom-pro-ip-rol' IDENTIFIED BY '{password}';


GRANT SELECT, INSERT, UPDATE ON `domain_date_provider_ip_spf` TO '{env}-dat-dom-pro-ip-spf-rol' IDENTIFIED BY '{password}';
GRANT SELECT, INSERT, UPDATE ON `domain_date_provider_ip_spf_store` TO '{env}-dat-dom-pro-ip-spf-rol' IDENTIFIED BY '{password}';


GRANT SELECT, INSERT, UPDATE ON `domain_date_provider_ip_dkim` TO '{env}-dat-dom-pro-ip-dkim' IDENTIFIED BY '{password}';
GRANT SELECT, INSERT, UPDATE ON `domain_date_provider_ip_dkim_store` TO '{env}-dat-dom-pro-ip-dkim' IDENTIFIED BY '{password}';


GRANT SELECT, INSERT, UPDATE ON `eslr` TO '{env}-eslr-rol' IDENTIFIED BY '{password}';

GRANT CREATE TEMPORARY TABLES ON *.* TO '{env}-agg-par' IDENTIFIED BY '{password}';
GRANT CREATE TEMPORARY TABLES ON *.* TO '{env}-dat-dom' IDENTIFIED BY '{password}';
GRANT CREATE TEMPORARY TABLES ON *.* TO '{env}-dat-dom-ru' IDENTIFIED BY '{password}';
GRANT CREATE TEMPORARY TABLES ON *.* TO '{env}-dat-dom-pro' IDENTIFIED BY '{password}';
GRANT CREATE TEMPORARY TABLES ON *.* TO '{env}-dat-dom-pro-ip-rol' IDENTIFIED BY '{password}';
GRANT CREATE TEMPORARY TABLES ON *.* TO '{env}-dat-dom-pro-ip-dkim' IDENTIFIED BY '{password}';
GRANT CREATE TEMPORARY TABLES ON *.* TO '{env}-dat-dom-pro-rol' IDENTIFIED BY '{password}';
GRANT CREATE TEMPORARY TABLES ON *.* TO '{env}-dat-dom-pro-sub-rol' IDENTIFIED BY '{password}';
GRANT CREATE TEMPORARY TABLES ON *.* TO '{env}-dat-dom-pro-ip-spf-rol' IDENTIFIED BY '{password}';
GRANT CREATE TEMPORARY TABLES ON *.* TO '{env}-eslr-rol' IDENTIFIED BY '{password}';
