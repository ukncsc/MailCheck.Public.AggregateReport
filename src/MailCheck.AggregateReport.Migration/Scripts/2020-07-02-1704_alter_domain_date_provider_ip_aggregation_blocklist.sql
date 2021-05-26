ALTER TABLE domain_date_provider_ip
    CHANGE blacklists_proxy blocklists_proxy bigint not null,
    CHANGE blacklists_hijackednetwork blocklists_hijackednetwork bigint not null,
    CHANGE blacklists_suspiciousnetwork blocklists_suspiciousnetwork bigint not null,
    CHANGE blacklists_endusernetwork blocklists_endusernetwork bigint not null,
    CHANGE blacklists_spamsource blocklists_spamsource bigint not null,
    CHANGE blacklists_malware blocklists_malware bigint not null,
    CHANGE blacklists_enduser blocklists_enduser bigint not null,
    CHANGE blacklists_bouncereflector blocklists_bouncereflector bigint not null;