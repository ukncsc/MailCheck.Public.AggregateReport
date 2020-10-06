ALTER TABLE eslr
    CHANGE proxy_blacklist proxy_blocklist bigint,
    CHANGE suspicious_network_blacklist suspicious_network_blocklist bigint,
    CHANGE hijacked_network_blacklist hijacked_network_blocklist bigint,
    CHANGE enduser_network_blacklist enduser_network_blocklist bigint,
    CHANGE spam_source_blacklist spam_source_blocklist bigint,
    CHANGE malware_blacklist malware_blocklist bigint,
    CHANGE enduser_blacklist enduser_blocklist bigint,
    CHANGE bounce_reflector_blacklist bounce_reflector_blocklist bigint;
