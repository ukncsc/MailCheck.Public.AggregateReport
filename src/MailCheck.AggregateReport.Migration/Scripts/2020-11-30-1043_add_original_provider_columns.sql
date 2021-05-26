ALTER TABLE domain_date_provider ADD original_provider VARCHAR(255);
ALTER TABLE domain_date_provider_subdomain ADD original_provider VARCHAR(255);
ALTER TABLE domain_date_provider_ip_dkim ADD original_provider VARCHAR(255);
ALTER TABLE domain_date_provider_ip_spf ADD original_provider VARCHAR(255);
ALTER TABLE domain_date_provider_rollup ADD original_provider VARCHAR(255);