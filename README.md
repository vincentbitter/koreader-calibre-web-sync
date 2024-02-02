# KOReader Calibre-Web Sync

Synchronize reader data from KOReader to Calibre-Web.

## Setup

The application is distributed as docker image:

```
docker run --restart=always -d -p 5000:5000 \
        --name=koreader \
        vincentbitter/koreader-calibre-web-sync:latest
```

**Notes:**

- This application was only tested with default authentication, so not with LDAP, OAuth or Reverse Proxy Authentication. As long as the normal login form is used, it should work, but especially 2FA is not supported.
- Make sure "Convert non-English characters in title and author while saving to disk" is disabled in the Feature Configuration section of the Basic Configuration page of Calibre-Web. Otherwise, different hashes might be calculated for books making it impossible to match.
- Use "Filename" as Document matching method in KOReader.

## How it works

Within KOReader, you can specify a custom sync server to synchronize reading progress. KOReader Calibre-Web Sync functions as a KOReader reading progress server. If the progress is 100% (last page is visited), it will mark the book as "Read" in Calibre-Web by logging in as a user via the web application.

## How to use

After the application is setup, make sure you have the correct URL to the docker container, that can be accessed from the e-reader. This can be a local address on the network (e.g. http://10.0.0.100:5000) or a public URL (https://stats.mykoreader.com).

Within KOReader, open a book and open the Tools menu and go to Progress Sync. Use the URL of your KOReader Calibre-Web Sync installation as Custom sync server (e.g. https://stats.mykoreader.com). Now Login is a bit different than normal, because you need to provide the username, password and URL of Calibre-Web. Therefore click "Register / Login" in the menu and use the following format as username: `<username>:<password>@<url>`. So if your Calibre-Web is hosted on https://my.calibre-web.com where you login with username `vincent` and password `secret`, then you need to use `vincent:secret@https://my.calibre-web.com` as your username. The password is ignored, so you can fill in a random value. Click on the Login button to validate your credentials.

If you change your Calibre-Web credentials, make sure you Logout and Login again to update the credentials.
