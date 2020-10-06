GRANT SELECT INTO S3 ON *.* TO '{env}-agg-par' IDENTIFIED BY '{password}';
GRANT INSERT, UPDATE ON `dkim_auth_result` TO '{env}-agg-par' IDENTIFIED BY '{password}';
GRANT INSERT, UPDATE ON `record` TO '{env}-agg-par' IDENTIFIED BY '{password}';
GRANT INSERT, UPDATE ON `spf_auth_result` TO '{env}-agg-par' IDENTIFIED BY '{password}';
GRANT INSERT, UPDATE ON `policy_override_reason` TO '{env}-agg-par' IDENTIFIED BY '{password}';
GRANT SELECT (id), INSERT, UPDATE ON `aggregate_report` TO '{env}-agg-par' IDENTIFIED BY '{password}';