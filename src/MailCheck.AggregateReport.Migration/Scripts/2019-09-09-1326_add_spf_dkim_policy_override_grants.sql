GRANT SELECT ON `spf_auth_result` TO '{env}_domstat' IDENTIFIED BY '{password}';
GRANT SELECT ON `dkim_auth_result` TO '{env}_domstat' IDENTIFIED BY '{password}';
GRANT SELECT ON `policy_override_reason` TO '{env}_domstat' IDENTIFIED BY '{password}';