﻿CREATE TABLE `eslr` (
  `record_id` BIGINT NOT NULL,
  `effective_date` DATE NOT NULL,
  `domain` VARCHAR(255) NOT NULL,
  `reverse_domain` VARCHAR(255) NOT NULL,
  `provider` VARCHAR(255) NOT NULL,
  `original_provider` VARCHAR(255),
  `reporter_org_name` VARCHAR(255) NOT NULL,
  `ip` VARCHAR(255) NOT NULL,
  `count` BIGINT,
  `disposition` VARCHAR(255),
  `dkim` VARCHAR(255),
  `spf` VARCHAR(255),
  `envelope_to` VARCHAR(255),
  `envelope_from` VARCHAR(255),
  `header_from` VARCHAR(255),
  `organisation_domain_from` VARCHAR(255),
  `spf_auth_results` VARCHAR(1000),
  `spf_pass_count` BIGINT,
  `spf_fail_count` BIGINT,
  `dkim_auth_results` VARCHAR(1000),
  `dkim_pass_count` BIGINT,
  `dkim_fail_count` BIGINT,
  `forwarded` BIGINT,
  `sampled_out` BIGINT,
  `trusted_forwarder` BIGINT,
  `mailing_list` BIGINT,
  `local_policy` BIGINT,
  `arc` BIGINT,
  `other_override_reason` BIGINT,
  `host_name` VARCHAR(255),
  `host_org_domain` VARCHAR(255),
  `host_provider` VARCHAR(255),
  `host_as_number` BIGINT,
  `host_as_description` VARCHAR(255),
  `host_country` VARCHAR(255),
  `proxy_blacklist` BIGINT,
  `suspicious_network_blacklist` BIGINT,
  `hijacked_network_blacklist` BIGINT,
  `enduser_network_blacklist` BIGINT,
  `spam_source_blacklist` BIGINT,
  `malware_blacklist` BIGINT,
  `enduser_blacklist` BIGINT,
  `bounce_reflector_blacklist` BIGINT,
  PRIMARY KEY (`reverse_domain`, `effective_date`, `record_id`));
  
  CREATE INDEX `dom_dat_pro` ON `eslr` (`reverse_domain`, `effective_date`, `provider`);
  CREATE INDEX `dom_dat_pro_ip` ON `eslr` (`reverse_domain`, `effective_date`, `provider`, `ip`);