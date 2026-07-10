#!/usr/bin/env python3
"""Serve terrain preview. Usage: python3 serve.py"""
import http.server
import os
import socket
import sys
import webbrowser

DIR = os.path.dirname(os.path.abspath(__file__))
PAGE = "/preview_standalone.html"
PORTS = [8765, 8766, 8767, 9080]


class Handler(http.server.SimpleHTTPRequestHandler):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, directory=DIR, **kwargs)

    def log_message(self, fmt, *args):
        print(f"[serve] {self.address_string()} {fmt % args}")


def port_free(port):
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        s.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        return s.connect_ex(("127.0.0.1", port)) != 0


def main():
    os.chdir(DIR)
    port = next((p for p in PORTS if port_free(p)), None)
    if port is None:
        print("[serve] ERROR: all ports busy:", PORTS, file=sys.stderr)
        sys.exit(1)
    url = f"http://127.0.0.1:{port}{PAGE}"
    print(f"[serve] Terrain preview at {url}")
    print("[serve] Ctrl+C to stop. Do NOT open the HTML file directly.")
    try:
        webbrowser.open(url)
    except Exception:
        pass
    http.server.HTTPServer(("127.0.0.1", port), Handler).serve_forever()


if __name__ == "__main__":
    main()
