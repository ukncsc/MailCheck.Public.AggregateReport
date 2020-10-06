CREATE TABLE public.asn_to_description_and_country
(
	asn bigint NOT NULL,
	description character varying NOT NULL,
	country character varying NOT NULL,
    CONSTRAINT asn_to_description_and_country_pkey PRIMARY KEY (asn)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

CREATE MATERIALIZED VIEW public.asn_to_description_and_country_mv
TABLESPACE pg_default
AS
 SELECT asn_to_description_and_country.asn,
    asn_to_description_and_country.description,
    asn_to_description_and_country.country
   FROM asn_to_description_and_country
WITH DATA;

CREATE UNIQUE INDEX idx_asn_to_description_and_country
    ON public.asn_to_description_and_country_mv USING btree
    (asn)
    TABLESPACE pg_default;

CREATE USER {env}_asntodescription WITH PASSWORD '{password}';

GRANT CONNECT ON DATABASE ip_intelligence TO {env}_asntodescription;

GRANT USAGE ON SCHEMA public TO {env}_asntodescription;

GRANT SELECT, UPDATE, INSERT, DELETE ON asn_to_description_and_country TO {env}_asntodescription;

GRANT SELECT, UPDATE, INSERT, DELETE ON asn_to_description_and_country_mv TO {env}_asntodescription;

ALTER MATERIALIZED VIEW asn_to_description_and_country_mv OWNER TO {env}_asntodescription;


