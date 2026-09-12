# NDC Unity Template

Unity project template maintained by CuongND, built and released via the
`unity-build-workflows` CI toolkit.

---

## CI / Build System

This project uses the
[unity-build-workflows](unity-build-workflows/README.md) toolkit (included as a
git submodule) for all CI builds. Unity operations run inside pinned Docker
containers on GitHub Actions — no local Unity installation is required for CI.

The pipeline has three layers, and each workflow answers one question.

| Workflow | Trigger | Question it answers |
|---|---|---|
| **CI / Validate & Test** (`01-ci.yml`) | push / PR | *Is this code safe to merge?* Validates, tests, reports. **Builds no player.** |
| **Build / Development** (`10-build-development.yml`) | manual | *Give me something to test with.* Android ships an **APK**. |
| **Build / Release** (`11-build-release.yml`) | manual | *Give me something we could ship.* Android ships a signed **AAB**. |
| **Release / Android** (`20-release-android.yml`) | manual | Promote a `release-android-aab` to Google Play. |
| **Release / iOS** (`21-release-ios.yml`) | manual | Promote a release IPA to App Store Connect. |
| **Release / WebGL** (`22-release-webgl.yml`) | manual | Promote a `release-webgl` to hosting. |

All of them call the same `unity-pipeline.yml` engine in the toolkit — the
split is user experience, not duplicated build logic.

### Build / Development vs Build / Release

Release is **not** Development with `environment=production`. It is signed,
store-shaped, and its artifacts are the immutable inputs to the Release
workflows. The artifact names keep the two apart, so a dev build can never be
mistaken for a release candidate:

| Platform | Development | Release |
|---|---|---|
| Android | `development-android-apk` | `release-android-aab` |
| iOS | `development-ios-xcodeproj` | `release-ios-xcodeproj` |
| WebGL | `development-webgl` | `release-webgl` |
| Windows | `development-windows` | `release-windows` |
| Linux | `development-linux` | `release-linux` |
| Linux (server) | `development-linux-server` | `release-linux-server` |

### Artifact promotion

Nothing is rebuilt between QA and production — the binary QA approved is the
binary that ships:

```
Build / Release  →  release-android-aab  →  Release / Android
                                             ├── internal testing
                                             ├── closed testing
                                             └── production   (approval)
```

Each Release workflow defaults to `start-phase` *after* the build, so it
promotes the stored artifact. Use a later phase to retry a failed upload
without rebuilding.

### Triggering builds manually

```bash
# Development APK
gh workflow run 10-build-development.yml --ref main -f platform=Android

# Release AAB for every platform in RELEASE_BUILD_PLATFORMS
gh workflow run 11-build-release.yml --ref main -f platform=All

# Promote that AAB to Google Play internal testing
gh workflow run 20-release-android.yml --ref main \
  -f build-version=1.4.2 -f package-name=com.company.game
```

> A `workflow_dispatch` workflow is only registered once it exists on the
> **default branch** (`main`). A newly added entry workflow is not dispatchable
> from a feature branch until it has been merged.

### Inputs

Inputs are grouped by who changes them; the group is the prefix on each field's
description. A normal build needs `GENERAL` and nothing else.

| Group | Inputs |
|---|---|
| `GENERAL` | platform, environment |
| `ANDROID` | `android-export` — **Build / Release only** |
| `QUALITY` | run-tests, test-mode |
| `CONTENT` | build-addressables |
| `UNITY` | unity-version, clean-build, define-symbols |
| `ADVANCED` | runner-type, build-engine, runner-labels (`auto` = use the repo variable) |

### The pipeline graph

```
Development / 01 / Resolve Build Config
Development / 02 / Quality Gate          ← builds wait for the tests
Development / 03 / Android / APK
Development / 04 / Android / Validate
Development / 07 / Final Report
Development / 08 / Notify Discord
```

Stages 03 and 04 are matrix jobs, so the graph contains exactly the platforms
selected. **iOS** needs a self-hosted macOS runner with the `macos-unity-xcode`
label; without one the iOS build reports `blocked` rather than failing the run.
**Windows** and **Linux** produce standalone artifacts and have no release
workflow, because this project has no distribution target for them.

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
