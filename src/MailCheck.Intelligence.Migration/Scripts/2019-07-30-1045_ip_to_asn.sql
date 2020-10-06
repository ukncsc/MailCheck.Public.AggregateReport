CREATE TABLE public.ip_address_to_asn
(
    ip_address cidr NOT NULL,
    asn bigint NOT NULL,
    CONSTRAINT ip_address_to_asn_pkey PRIMARY KEY (ip_address, asn)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

CREATE MATERIALIZED VIEW public.ip_address_to_asn_mv
TABLESPACE pg_default
AS
 SELECT ip_address_to_asn.ip_address,
    ip_address_to_asn.asn
   FROM ip_address_to_asn
WITH DATA;

CREATE UNIQUE INDEX idx_ip_address_asn
    ON public.ip_address_to_asn_mv USING btree
    (ip_address cidr_ops, asn)
    TABLESPACE pg_default;

CREATE USER {env}_iptoasn WITH PASSWORD '{password}';

GRANT CONNECT ON DATABASE ip_intelligence TO {env}_iptoasn;

GRANT USAGE ON SCHEMA public TO {env}_iptoasn;

GRANT SELECT, UPDATE, INSERT, DELETE ON ip_address_to_asn TO {env}_iptoasn;

GRANT SELECT, UPDATE, INSERT, DELETE ON ip_address_to_asn_mv TO {env}_iptoasn;

ALTER MATERIALIZED VIEW ip_address_to_asn_mv OWNER TO {env}_iptoasn;


