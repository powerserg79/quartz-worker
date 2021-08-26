# Worker

This application is a template to assist in creating a workers service that can schedule jobs. It uses quartz.net to manage the jobs.

## Run in Docker

The project has a dockerfile defined setup a docker image

To test the docker container, first build an image

```bash
./build.sh
```

And then run the container based on that image

```bash
./run.sh
```

The shell output will should log out two example jobs, `Notifation Job` and `Log Job` triggered at intervals.