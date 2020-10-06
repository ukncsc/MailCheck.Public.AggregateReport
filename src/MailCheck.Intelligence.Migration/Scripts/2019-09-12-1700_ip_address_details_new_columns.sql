ALTER TABLE public.ip_address_details ADD COLUMN asn_updated timestamp without time zone;
ALTER TABLE public.ip_address_details ADD COLUMN reverse_dns_updated timestamp without time zone;
ALTER TABLE public.ip_address_details ADD COLUMN blacklist_updated timestamp without time zone;