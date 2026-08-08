# NDC Unity Template

Unity project template maintained by CuongND, built and released via the
`unity-build-workflows` CI toolkit.

---

## CI / Build System

This project uses the
[unity-build-workflows](unity-build-workflows/README.md) toolkit (included as a
git submodule) for all CI builds. Unity operations run inside pinned Docker
containers on GitHub Actions — no local Unity installation is required for CI.

The active build workflow is **`unity-build.yml`** (explicit-platform-jobs flow).
Each platform is a separate named job in the GitHub Actions UI — independently
retryable and independently colour-coded.

See [unity-build-workflows/docs/EXPLICIT\_PLATFORM\_FLOW.md](unity-build-workflows/docs/EXPLICIT_PLATFORM_FLOW.md)
for a full guide to the job graph, inputs, activation, and platform selection rules.

### Triggering builds manually

```bash
# Android
gh workflow run unity-build.yml \
  --repo Cuvara/NDC-Unity-Template \
  --ref main \
  -f platform=Android

# WebGL
gh workflow run unity-build.yml \
  --repo Cuvara/NDC-Unity-Template \
  --ref main \
  -f platform=WebGL

# Linux64
gh workflow run unity-build.yml \
  --repo Cuvara/NDC-Unity-Template \
  --ref main \
  -f platform=Linux64

# Linux Dedicated Server
gh workflow run unity-build.yml \
  --repo Cuvara/NDC-Unity-Template \
  --ref main \
  -f platform=LinuxServer

# All platforms at once
gh workflow run unity-build.yml \
  --repo Cuvara/NDC-Unity-Template \
  --ref main \
  -f platform=All
```

Supported platforms: **Android**, **WebGL**, **Linux64**, **LinuxServer**.
**iOS** requires a registered self-hosted macOS runner with the
`macos-unity-xcode` label — it is **blocked** until one is provisioned (see
[SELF\_HOSTED\_MACOS\_RUNNER.md](unity-build-workflows/docs/SELF_HOSTED_MACOS_RUNNER.md),
[EXPLICIT\_PLATFORM\_FLOW.md § iOS](unity-build-workflows/docs/EXPLICIT_PLATFORM_FLOW.md#6-ios-build--special-requirements)
and
[GITHUB\_ACTIONS\_BUILD\_RUNBOOK.md § 10](unity-build-workflows/docs/GITHUB_ACTIONS_BUILD_RUNBOOK.md#10-iosmacos-runner-limitations)).

### Key dispatch inputs

| Input | Default | Description |
|---|---|---|
| `platform` | `All` | `All`, `Android`, `WebGL`, `Linux64`, `LinuxServer`, `iOS` |
| `run-tests` | `false` | Run Unity tests before builds |
| `build-addressables` | `false` | Build Addressables catalog before platform builds |
| `environment` | `production` | `production`, `staging`, `development` |
| `runner-mode` | `docker` | `docker`, `self-hosted-windows` |
| `clean-build` | `false` | Force full `Library/` cache delete |

Full input reference: [EXPLICIT\_PLATFORM\_FLOW.md § 2](unity-build-workflows/docs/EXPLICIT_PLATFORM_FLOW.md#2-workflow-dispatch-inputs).

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
gh secret list --repo Cuvara/NDC-Unity-Template \
  | grep -E 'UNITY_LICENSE|UNITY_EMAIL|UNITY_PASSWORD'
```

---

## Downloading Build Artifacts

Build artifacts are retained for 14 days.

```bash
# List recent builds
gh run list --repo Cuvara/NDC-Unity-Template \
  --workflow build.yml --limit 10

# Download artifacts from a specific run
gh run download <RUN_ID> --repo Cuvara/NDC-Unity-Template
```

---

## Documentation

| Document | Description |
|---|---|
| [unity-build-workflows/docs/EXPLICIT\_PLATFORM\_FLOW.md](unity-build-workflows/docs/EXPLICIT_PLATFORM_FLOW.md) | **New** — explicit-platform-jobs flow: job graph, dispatch inputs, activation, platform selection, iOS requirements |
| [unity-build-workflows/docs/UNITY\_PERSONAL\_DOCKER\_LICENSE.md](unity-build-workflows/docs/UNITY_PERSONAL_DOCKER_LICENSE.md) | Unity Personal/free Docker licensing — `personal-combined` strategy, secret setup, troubleshooting |
| [unity-build-workflows/docs/UNITY\_VERSION\_UPGRADE.md](unity-build-workflows/docs/UNITY_VERSION_UPGRADE.md) | Step-by-step Unity version upgrade checklist |
| [unity-build-workflows/docs/GITHUB\_ACTIONS\_BUILD\_RUNBOOK.md](unity-build-workflows/docs/GITHUB_ACTIONS_BUILD_RUNBOOK.md) | Operational runbook — triggering builds, reading logs, artifacts, common errors |
| [unity-build-workflows/docs/SELF\_HOSTED\_MACOS\_RUNNER.md](unity-build-workflows/docs/SELF_HOSTED_MACOS_RUNNER.md) | Provisioning a `macos-unity-xcode` self-hosted runner for iOS builds (Xcode, Unity iOS module, activation) |
| [unity-build-workflows/README.md](unity-build-workflows/README.md) | CI toolkit — architecture, workflows, image variants |
