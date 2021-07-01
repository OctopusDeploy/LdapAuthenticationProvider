FROM osixia/openldap

LABEL maintainer="devops@occtopus.com"

ENV LDAP_ORGANISATION="Domain 1" \
    LDAP_DOMAIN="domain1.local"

COPY bootstrap.ldif /container/service/slapd/assets/config/bootstrap/ldif/50-bootstrap.ldif
