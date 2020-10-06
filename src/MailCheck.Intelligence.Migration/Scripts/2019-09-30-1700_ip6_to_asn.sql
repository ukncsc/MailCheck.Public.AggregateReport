CREATE TABLE public.ip6_address_to_asn
(
    ip_address cidr NOT NULL,
    asn bigint NOT NULL,
    CONSTRAINT ip6_address_to_asn_pkey PRIMARY KEY (ip_address, asn)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

CREATE MATERIALIZED VIEW public.ip6_address_to_asn_mv
TABLESPACE pg_default
AS
 SELECT ip6_address_to_asn.ip_address,
    ip6_address_to_asn.asn
   FROM ip6_address_to_asn
WITH DATA;

CREATE UNIQUE INDEX idx_ip6_address_asn
    ON public.ip6_address_to_asn_mv USING btree
    (ip_address cidr_ops, asn)
    TABLESPACE pg_default;

GRANT SELECT, UPDATE, INSERT, DELETE ON ip6_address_to_asn TO {env}_iptoasn;

GRANT SELECT, UPDATE, INSERT, DELETE ON ip6_address_to_asn_mv TO {env}_iptoasn;
GRANT SELECT ON ip6_address_to_asn_mv TO {env}_ipintelapi;

ALTER MATERIALIZED VIEW ip6_address_to_asn_mv OWNER TO {env}_iptoasn;


