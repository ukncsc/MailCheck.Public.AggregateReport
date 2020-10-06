CREATE TABLE public.ip_address_details
(
    as_number integer,
    ip_address inet NOT NULL,
    description character varying(300) COLLATE pg_catalog."default",
    country_code character(2) COLLATE pg_catalog."default",
    date date NOT NULL,
    reverse_dns_data json,
    blacklist_data json,
    CONSTRAINT ip_address_details_pkey PRIMARY KEY (ip_address, date)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

CREATE USER {env}_ipintelapi WITH PASSWORD '{password}';

GRANT CONNECT ON DATABASE ip_intelligence TO {env}_ipintelapi;

GRANT USAGE ON SCHEMA public TO {env}_ipintelapi;

GRANT SELECT, UPDATE, INSERT, DELETE ON ip_address_details TO {env}_ipintelapi;

GRANT {env}_iptoasn TO CURRENT_USER;
GRANT {env}_asntodescription TO CURRENT_USER;
GRANT {env}_pubsuf TO CURRENT_USER;

GRANT SELECT ON public_suffix_mv TO {env}_ipintelapi;
GRANT SELECT ON asn_to_description_and_country_mv TO {env}_ipintelapi;
GRANT SELECT ON ip_address_to_asn_mv TO {env}_ipintelapi;