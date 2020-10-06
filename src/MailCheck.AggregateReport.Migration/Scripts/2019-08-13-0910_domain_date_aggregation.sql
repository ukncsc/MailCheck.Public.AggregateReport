CREATE TABLE `domain_date` (
  `domain` VARCHAR(255) NOT NULL,
  `date` DATE NOT NULL,
  `spf_pass_dkim_pass_none` BIGINT NOT NULL,
  `spf_pass_dkim_fail_none` BIGINT NOT NULL,
  `spf_fail_dkim_pass_none` BIGINT NOT NULL,
  `spf_fail_dkim_fail_none` BIGINT NOT NULL,
  `spf_pass_dkim_pass_quarantine` BIGINT NOT NULL,
  `spf_pass_dkim_fail_quarantine` BIGINT NOT NULL,
  `spf_fail_dkim_pass_quarantine` BIGINT NOT NULL,
  `spf_fail_dkim_fail_quarantine` BIGINT NOT NULL,
  `spf_pass_dkim_pass_reject` BIGINT NOT NULL,
  `spf_pass_dkim_fail_reject` BIGINT NOT NULL,
  `spf_fail_dkim_pass_reject` BIGINT NOT NULL,
  `spf_fail_dkim_fail_reject` BIGINT NOT NULL,
  PRIMARY KEY (`domain`, `date`));

  CREATE TABLE `domain_date_store` (
  `record_id` BIGINT NOT NULL,
  PRIMARY KEY (`record_id`));