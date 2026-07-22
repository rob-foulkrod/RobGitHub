# Git Commands

Git is the **version control system** that tracks the history of your project.
Below are the core commands we use in class, grouped by what you are trying to do.
Each command includes a short explanation and an example.

!!! info "How to read the examples"
    Lines starting with `#` are comments. The actual command follows on the next
    line. You type these in a terminal from inside your project folder.

## Getting started

### `git init`

Creates a brand new, empty Git repository in the current folder. This is the very
first step when you want to start tracking a project.

```bash
# Turn the current folder into a Git repository
git init
```

### `git clone`

Copies an existing remote repository (for example from GitHub) down to your
computer, including its full history.

```bash
# Download a copy of a GitHub repository
git clone https://github.com/rob-foulkrod/motown.git
```

## Saving your work

The everyday Git workflow is a loop: **change files → stage them → commit them**.

### `git status`

Shows the current state of your working directory and staging area: which branch
you are on, which files have changed, and which changes are staged.

```bash
# See what has changed and what is staged
git status
```

### `git add`

Puts new or changed files into the **staging area (index)** so they will be
included in your next commit.

```bash
# Stage a single file
git add README.md

# Stage everything that changed
git add .
```

### `git commit`

Saves a new commit (a snapshot) of everything currently staged. A commit needs a
message describing the change, supplied with `-m`.

```bash
# Save a commit with a message
git commit -m "Add git commands to the docs"
```

### `git log`

Shows the history of commits, newest first. Useful for reviewing what has happened.

```bash
# View the full commit history
git log

# A compact, one-line-per-commit view
git log --oneline
```

## Working with branches

Branches let you work on changes in isolation without affecting the main line of
development.

### `git switch`

Switches between branches, or creates a new branch and switches to it. It is the
modern, clearer replacement for `git checkout` when changing branches.

```bash
# Switch to an existing branch
git switch main

# Create a new branch and switch to it
git switch -c feature/new-docs
```

### `git merge`

Combines the changes from one branch into your current branch. When there is a
straight line of history, Git can do a simple **fast-forward** merge.

```bash
# While on main, merge a feature branch into it
git switch main
git merge feature/new-docs
```

## Connecting to GitHub

### `git remote`

Manages the connections ("remotes") to other copies of your repository, such as the
one hosted on GitHub. The default remote is usually named `origin`.

```bash
# List the remotes you are connected to
git remote -v
```

### `git push`

Uploads your local commits to a remote repository so others (and GitHub) can see
them.

```bash
# Push your current branch's commits to GitHub
git push
```

### `git pull`

Downloads commits from the remote and merges them into your current branch — the
counterpart to `git push`.

```bash
# Get the latest changes from GitHub
git pull
```

## Quick reference

| Command | What it does |
| --- | --- |
| `git init` | Create a new repository |
| `git clone` | Copy a remote repository locally |
| `git status` | Show what has changed |
| `git add` | Stage changes for the next commit |
| `git commit` | Save a snapshot of staged changes |
| `git log` | View the commit history |
| `git switch` | Change or create branches |
| `git merge` | Combine branches together |
| `git remote` | Manage connections to remote repos |
| `git push` | Upload commits to a remote |
| `git pull` | Download and merge remote commits |
