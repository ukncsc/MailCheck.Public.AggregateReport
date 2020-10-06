CREATE TABLE `domain_date_provider_ip_dkim` (
  `domain` VARCHAR(255) NOT NULL,
  `date` DATE NOT NULL,
  `provider` VARCHAR(255) NOT NULL,
  `ip` VARCHAR(255) NOT NULL,
  `dkim_domain` VARCHAR(255) NOT NULL,
  `dkim_selector` VARCHAR(255) NOT NULL,
  `dkim_pass` BIGINT NOT NULL,
  `dkim_fail` BIGINT NOT NULL,
  PRIMARY KEY (`domain`, `provider`, `date`, `ip`, `dkim_domain`, `dkim_selector`));

  CREATE TABLE `domain_date_provider_ip_dkim_store` (
  `record_id` BIGINT NOT NULL,
  PRIMARY KEY (`record_id`));