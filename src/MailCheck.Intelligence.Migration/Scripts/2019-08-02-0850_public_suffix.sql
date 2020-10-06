CREATE TABLE public.public_suffix
(
	suffix character varying NOT NULL,
    CONSTRAINT public_suffix_pkey PRIMARY KEY (suffix)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

CREATE MATERIALIZED VIEW public.public_suffix_mv
TABLESPACE pg_default
AS
 SELECT suffix
   FROM public_suffix
WITH DATA;

CREATE UNIQUE INDEX idx_public_suffix
    ON public.public_suffix_mv USING btree
    (suffix)
    TABLESPACE pg_default;

CREATE USER {env}_pubsuf WITH PASSWORD '{password}';

GRANT CONNECT ON DATABASE ip_intelligence TO {env}_pubsuf;

GRANT USAGE ON SCHEMA public TO {env}_pubsuf;

GRANT SELECT, UPDATE, INSERT, DELETE ON public_suffix TO {env}_pubsuf;

GRANT SELECT, UPDATE, INSERT, DELETE ON public_suffix_mv TO {env}_pubsuf;

ALTER MATERIALIZED VIEW public_suffix_mv OWNER TO {env}_pubsuf;
