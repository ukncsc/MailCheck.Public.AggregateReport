ALTER TABLE `aggregatereport`.`policy_override_reason` 
CHANGE COLUMN `policy_override` `policy_override` ENUM('forwarded', 'sampled_out', 'trusted_forwarder', 'mailing_list', 'local_policy', 'other') NULL DEFAULT NULL ;
