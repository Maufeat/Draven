# Draven (based on Poro from Snowl)

## Setting up your LeagueClient

Add the following Region to your `region_data:` in `system.yaml` (Replace the TEST: entry if already exists) in your League of Legends Root:

You'll most likely have to edit the `system.yaml` in `\RADS\projects\league_client\releases\{highest version}\deploy` too 

```
  TEST:
    available_locales:
    - en_GB
    default_locale: en_GB
    rso:
      allow_lq_fallback: true
      kount:
        collector: prod02.kaxsdc.com
        merchant: '108000'
      token: eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJhdWQiOiJodHRwOi8vMTI3LjAuMC4xOjgwODAvdG9rZW4iLCJzdWIiOiJsb2wiLCJpc3MiOiJsb2wiLCJleHAiOjE1MTk1ODgzNDQsImlhdCI6MTQ5NDgyMTE4NiwianRpIjoiNzRiY2Q0YjEtYTQ3Mi00MjU2LWE0MDItMzY5Zjg3ZjhkZDNjIn0.I9YtnXgUL3h23KuncW2P3vOdXjjafWmWW6p0vczPXh0
      rso_platform_id: NA1
    servers:
      account_recovery:
        forgot_password_url: https://recovery.riotgames.com/{{lang}}/forgot-password?region={{region}}
        forgot_username_url: https://recovery.riotgames.com/{{lang}}/forgot-username?region={{region}}
      chat:
        allow_self_signed_cert: true
        chat_host: chat.na1.lol.riotgames.com
        chat_port: 5223
      discoverous_service_location: lolriot.pdx2.na1           
      email_verification:
        external_url: https://email-verification.riotgames.com/api
      entitlements:
        entitlements_url: https://entitlements.auth.riotgames.com/api/token/v1
      lcds:
        lcds_host: 127.0.0.1
        lcds_port: 2099
        login_queue_url: http://127.0.0.1:8080
        use_tls: false
      license_agreement_urls:
        terms_of_use: http://127.0.0.1:8080/{language}/legal/termsofuse
      payments:
        payments_host: https://plstore.na1.lol.riotgames.com
      prelogin_config:
        prelogin_config_url: https://prod.config.patcher.riotgames.com
      rms:
        rms_heartbeat_interval_seconds: 60
        rms_url: wss://eu.edge.rms.si.riotgames.com:443
      service_status:
        api_url: https://status.leagueoflegends.com/shards/euw/synopsis
        human_readable_status_url: https://status.leagueoflegends.com/#euw
      store:
        store_url: https://store.euw1.lol.riotgames.com
    web_region: euw
``` 
