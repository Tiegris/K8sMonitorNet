{{/*
Expand the name of the chart.
*/}}
{{- define "k8s-monitor-net.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "k8s-monitor-net.fullname" -}}
{{- if .Values.fullnameOverride }}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- $name := default .Chart.Name .Values.nameOverride }}
{{- if contains $name .Release.Name }}
{{- .Release.Name | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" }}
{{- end }}
{{- end }}
{{- end }}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "k8s-monitor-net.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Common labels
*/}}
{{- define "k8s-monitor-net.labels" -}}
helm.sh/chart: {{ include "k8s-monitor-net.chart" . }}
{{ include "k8s-monitor-net.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{/*
Selector labels
*/}}
{{- define "k8s-monitor-net.selectorLabels" -}}
app.kubernetes.io/name: {{ include "k8s-monitor-net.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}

{{/*
Create the name of the service account to use
*/}}
{{- define "k8s-monitor-net.serviceAccountName" -}}
{{- if .Values.serviceAccount.create }}
{{- default (include "k8s-monitor-net.fullname" .) .Values.serviceAccount.name }}
{{- else }}
{{- default "default" .Values.serviceAccount.name }}
{{- end }}
{{- end }}

{{- define "helpers.list-envs" }}
{{- range $key, $val := .Values.extraEnvs }}
- name: {{ $key | quote }}
  value: {{ $val }}
{{- end }}
{{- end }}

{{- define "helpers.list-appSettings" }}
{{- range $key, $val := .Values.appSettings }}
- name: {{ $key | quote }}
  value: {{ $val }}
{{- end }}
{{- end }}