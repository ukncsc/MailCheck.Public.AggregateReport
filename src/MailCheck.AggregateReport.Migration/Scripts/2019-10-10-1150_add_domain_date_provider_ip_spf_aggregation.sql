CREATE TABLE `domain_date_provider_ip_spf` (
  `domain` VARCHAR(255) NOT NULL,
  `date` DATE NOT NULL,
  `provider` VARCHAR(255) NOT NULL,
  `ip` VARCHAR(255) NOT NULL,
  `spf_domain` VARCHAR(255) NOT NULL,
  `spf_pass` BIGINT NOT NULL,
  `spf_fail` BIGINT NOT NULL,
  PRIMARY KEY (`domain`, `provider`, `date`, `ip`, `spf_domain`));

  CREATE TABLE `domain_date_provider_ip_spf_store` (
  `record_id` BIGINT NOT NULL,
  PRIMARY KEY (`record_id`));