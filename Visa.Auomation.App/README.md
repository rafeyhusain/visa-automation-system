# Web Page Auto Filler – Automation Utility

**Web Page Auto Filler** is a C#-based desktop automation tool designed to automatically fill and submit HTML forms in a browser (e.g., login forms, registration, visa applications, etc.). It uses Selenium WebDriver with Chrome and supports a JSON-based workflow to define and control interactions with form elements.

---

## Features

* Automatically fill fields like textboxes, selects, checkboxes, hidden fields, and more.
* Submit forms via button clicks.
* Inject values into hidden fields using JavaScript.
* Waits for elements to be visible before interacting.
* JSON-based workflows for easy reusability.
* Built-in error logging with recursive exception detail.
* Use [proxies](https://www.scraperapi.com/blog/best-10-free-proxies-and-free-proxy-lists-for-web-scraping/) to avoid IP blocking

---

## Usage Example

Fill a login form, solve CAPTCHA (manually/placeholder), and click the login button:

```json
[
  {
    "id": "1",
    "type": "fill",
    "title": "Fill login form",
    "data": [
      { "id": "email_id", "value": "a@b.com", "type": "email" },
      { "id": "pwd_id", "value": "123", "type": "password" },
      { "id": "hidden_token", "value": "xyz123", "type": "hidden" }
    ]
  },
  {
    "id": "2",
    "type": "cloudflare-captcha",
    "title": "Solve CAPTCHA"
  },
  {
    "id": "3",
    "type": "button-click",
    "title": "Click Login",
    "data": [
      { "id": "login_id" }
    ]
  }
]
```

---

## Configuration Files

### `workflow.json`

Defines the steps (tasks) to be performed. Each task includes:

| Field   | Description                                   |
| ------- | --------------------------------------------- |
| `id`    | Unique identifier for task                    |
| `type`  | Type of task (`fill`, `button-click`, etc.)   |
| `title` | Human-readable title                          |
| `data`  | Array of field objects (for applicable types) |

#### Supported Field Types

| Type       | Description                             |
| ---------- | --------------------------------------- |
| `textbox`  | Default text input                      |
| `text`     | Alias for textbox                       |
| `email`    | Email input                             |
| `password` | Password field                          |
| `number`   | Numeric input                           |
| `date`     | Date picker                             |
| `textarea` | Multi-line input                        |
| `select`   | Dropdown (select by value or text)      |
| `checkbox` | Checkbox (true/false as string)         |
| `radio`    | Radio button                            |
| `hidden`   | Hidden field (via JavaScript injection) |

---

### `appsettings.json`

Contains runtime settings for launching and controlling the browser.

```json
{
    "TimeoutSeconds": 10,
    "PollingIntervalMilliseconds": 500
}
```

#### Property Reference

| Property              | Description                                    |
| --------------------- | ---------------------------------------------- |
| `TimeoutSeconds`    |                |
| `PollingIntervalMilliseconds`            |              |

---

## Error Handling

The application uses:

```csharp
void Log(string message);
void Log(string message, Exception ex);
```

* All errors are displayed via `MessageBox.Show`.
* Inner exceptions are recursively appended with new lines.

# Web Form Auto Filler – `workflow.json` Guide

This document explains how to structure the `workflow.json` used by the Web Form Auto Filler utility. The JSON defines a series of tasks that simulate user actions on web forms such as filling inputs, selecting dropdown options, checking checkboxes, and solving CAPTCHA.

---

## 📄 JSON Structure

The `workflow.json` must be an array of task objects. Each task object contains:

* `id`: A unique identifier for the task
* `type`: The task type (e.g. `fill`, `button-click`)
* `title`: A brief title for the task
* `data`: (Optional) An array of field descriptors used in `fill` or `button-click` tasks

---

## 🔧 Supported Task Types

### 1. `fill`

Fills form fields with the specified values.

```json
{
  "id": "1",
  "type": "fill",
  "title": "Fill login form",
  "data": [
    {
      "id": "email_id",
      "value": "a@b.com",
      "type": "email"
    },
    {
      "id": "pwd_id",
      "value": "123",
      "type": "password"
    },
    {
      "id": "hidden_token",
      "value": "xyz123",
      "type": "hidden"
    },
    {
      "id": "visaType",
      "value": "tourist",
      "type": "select"
    },
    {
      "id": "termsCheckbox",
      "value": "true",
      "type": "checkbox"
    },
    {
      "id": "genderMale",
      "value": "true",
      "type": "radio"
    },
    {
      "id": "comments",
      "value": "Looking forward to my visit.",
      "type": "textarea"
    }
  ]
}
```

### Supported Field `type` Values:

| Type       | Description                              |
| ---------- | ---------------------------------------- |
| `textbox`  | Default text input (alias for `text`)    |
| `text`     | Standard text input                      |
| `email`    | Email input field                        |
| `password` | Password input field                     |
| `number`   | Number input                             |
| `date`     | Date picker input                        |
| `hidden`   | Hidden field (uses JavaScript injection) |
| `textarea` | Multi-line input                         |
| `select`   | Dropdown selection (by value or text)    |
| `checkbox` | Checkbox (set via boolean string)        |
| `radio`    | Radio button (select if not already)     |

---

### 2. `cloudflare-captcha`

Placeholder for solving Cloudflare CAPTCHA (to be implemented or integrated).

```json
{
  "id": "2",
  "type": "cloudflare-captcha",
  "title": "Solve Cloudflare CAPTCHA"
}
```

---

### 3. `button-click`

Clicks a button by element ID.

```json
{
  "id": "3",
  "type": "button-click",
  "title": "Submit login form",
  "data": [
    {
      "id": "login_id"
    }
  ]
}
```

---

## 🧪 Tips for Setup

* Ensure all element IDs in your form match the `id` fields in `workflow.json`.
* For `select`, `checkbox`, and `radio`, values must exactly match what’s on the HTML page.
* Use `"true"` or `"false"` (as strings) for `checkbox` and `radio`.
* Hidden fields will be filled using JavaScript due to DOM restrictions.

---

## 🐞 Error Logging

The app uses the following logging methods:

```csharp
void Log(string message)
void Log(string message, Exception ex)
```

* Logs are shown via `MessageBox.Show`.
* Exception logs recursively append all `ex.InnerException.Message` lines.

---

## ✅ Example Full Workflow

```json
[
  {
    "id": "1",
    "type": "fill",
    "title": "Fill login form",
    "data": [
      {
        "id": "email_id",
        "value": "a@b.com",
        "type": "email"
      },
      {
        "id": "pwd_id",
        "value": "123",
        "type": "password"
      },
      {
        "id": "hidden_token",
        "value": "abc123",
        "type": "hidden"
      }
    ]
  },
  {
    "id": "2",
    "type": "cloudflare-captcha",
    "title": "Solve Cloudflare CAPTCHA"
  },
  {
    "id": "3",
    "type": "button-click",
    "title": "Submit login form",
    "data": [
      {
        "id": "login_id"
      }
    ]
  }
]
```
