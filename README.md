# NDC Unity Template

Unity project template maintained by CuongND, built and released via the
`unity-build-workflows` CI toolkit.

---

## CI / Build System

This project uses the
[unity-build-workflows](unity-build-workflows/README.md) toolkit (included as a
git submodule) for all CI builds. Unity operations run inside pinned Docker
containers on GitHub Actions — no local Unity installation is required for CI.

### Triggering builds manually

```bash
# Android (default)
gh workflow run build.yml \
  --repo dyCuong03/NDC-Unity-Template \
  --ref main \
  -f platform=Android

# WebGL
gh workflow run build.yml \
  --repo dyCuong03/NDC-Unity-Template \
  --ref main \
  -f platform=WebGL

# Linux64
gh workflow run build.yml \
  --repo dyCuong03/NDC-Unity-Template \
  --ref main \
  -f platform=Linux64

# All Docker platforms at once
gh workflow run build.yml \
  --repo dyCuong03/NDC-Unity-Template \
  --ref main \
  -f platform=All
```

Supported platforms: **Android**, **WebGL**, **Linux64**, **LinuxServer**.
iOS is deferred (requires a self-hosted macOS runner — see
[docs/GITHUB\_ACTIONS\_BUILD\_RUNBOOK.md](unity-build-workflows/docs/GITHUB_ACTIONS_BUILD_RUNBOOK.md#10-iosmacos-runner-limitations)).

### Unity version

Current version: **6000.0.26f1** — defined in
`ProjectSettings/ProjectVersion.txt` (single source of truth).

To upgrade Unity, follow the checklist in
[unity-build-workflows/docs/UNITY\_VERSION\_UPGRADE.md](unity-build-workflows/docs/UNITY_VERSION_UPGRADE.md).

---

## Required Secrets

Configure in `Settings → Secrets and variables → Actions`:

| Secret | Required | Purpose |
|---|---|---|
| `UNITY_LICENSE` | Yes | Raw `.ulf` file contents |
| `UNITY_EMAIL` | Yes | Unity account email |
| `UNITY_PASSWORD` | Yes | Unity account password |
| `ANDROID_KEYSTORE_BASE64` | Optional | Android signing |
| `ANDROID_KEYSTORE_PASS` | Optional | Android signing |
| `ANDROID_KEY_ALIAS` | Optional | Android signing |
| `ANDROID_KEY_PASS` | Optional | Android signing |
| `DISCORD_WEBHOOK_URL` | Optional | Discord build notifications |

All three Unity license secrets (`UNITY_LICENSE`, `UNITY_EMAIL`,
`UNITY_PASSWORD`) must be set together. See
[unity-build-workflows/docs/UNITY\_PERSONAL\_DOCKER\_LICENSE.md](unity-build-workflows/docs/UNITY_PERSONAL_DOCKER_LICENSE.md)
for setup instructions.

Verify secrets are present:
```bash
gh secret list --repo dyCuong03/NDC-Unity-Template \
  | grep -E 'UNITY_LICENSE|UNITY_EMAIL|UNITY_PASSWORD'
```

---

## Downloading Build Artifacts

Build artifacts are retained for 14 days.

```bash
# List recent builds
gh run list --repo dyCuong03/NDC-Unity-Template \
  --workflow build.yml --limit 10

# Download artifacts from a specific run
gh run download <RUN_ID> --repo dyCuong03/NDC-Unity-Template
```

---

## Documentation

| Document | Description |
|---|---|
| [unity-build-workflows/docs/UNITY\_PERSONAL\_DOCKER\_LICENSE.md](unity-build-workflows/docs/UNITY_PERSONAL_DOCKER_LICENSE.md) | Unity Personal/free Docker licensing — `personal-combined` strategy, secret setup, troubleshooting |
| [unity-build-workflows/docs/UNITY\_VERSION\_UPGRADE.md](unity-build-workflows/docs/UNITY_VERSION_UPGRADE.md) | Step-by-step Unity version upgrade checklist |
| [unity-build-workflows/docs/GITHUB\_ACTIONS\_BUILD\_RUNBOOK.md](unity-build-workflows/docs/GITHUB_ACTIONS_BUILD_RUNBOOK.md) | Operational runbook — triggering builds, reading logs, artifacts, common errors |
| [unity-build-workflows/README.md](unity-build-workflows/README.md) | CI toolkit — architecture, workflows, image variants |
