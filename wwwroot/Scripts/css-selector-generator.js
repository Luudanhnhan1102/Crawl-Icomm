﻿/*
CSS Selector Generator, v1.1.0
by Riki Fridrich <riki@fczbkk.com> (http://fczbkk.com)
https://github.com/fczbkk/css-selector-generator/
*/(function () { var a, b, c = [].indexOf || function (a) { for (var b = 0, c = this.length; b < c; b++)if (b in this && this[b] === a) return b; return -1 }; a = function () { function a(a) { null == a && (a = {}), this.options = {}, this.setOptions(this.default_options), this.setOptions(a) } return a.prototype.default_options = { selectors: ["id", "class", "tag", "nthchild"] }, a.prototype.setOptions = function (a) { var b, c, d; null == a && (a = {}), c = []; for (b in a) d = a[b], this.default_options.hasOwnProperty(b) ? c.push(this.options[b] = d) : c.push(void 0); return c }, a.prototype.isElement = function (a) { return !(1 !== (null != a ? a.nodeType : void 0)) }, a.prototype.getParents = function (a) { var b, c; if (c = [], this.isElement(a)) for (b = a; this.isElement(b);)c.push(b), b = b.parentNode; return c }, a.prototype.getTagSelector = function (a) { return this.sanitizeItem(a.tagName.toLowerCase()) }, a.prototype.sanitizeItem = function (a) { var b; return b = a.split("").map(function (a) { return ":" === a ? "\\" + ":".charCodeAt(0).toString(16).toUpperCase() + " " : /[ !"#$%&'()*+,.\/;<=>?@\[\\\]^`{|}~]/.test(a) ? "\\" + a : escape(a).replace(/\%/g, "\\") }), b.join("") }, a.prototype.getIdSelector = function (a) { var b, c; return b = a.getAttribute("id"), null == b || "" === b || /\s/.exec(b) || /^\d/.exec(b) || (c = "#" + this.sanitizeItem(b), 1 !== a.ownerDocument.querySelectorAll(c).length) ? null : c }, a.prototype.getClassSelectors = function (a) { var b, c, d; return d = [], b = a.getAttribute("class"), null != b && (b = b.replace(/\s+/g, " "), b = b.replace(/^\s|\s$/g, ""), "" !== b && (d = function () { var a, d, e, f; for (e = b.split(/\s+/), f = [], a = 0, d = e.length; a < d; a++)c = e[a], f.push("." + this.sanitizeItem(c)); return f }.call(this))), d }, a.prototype.getAttributeSelectors = function (a) { var b, d, e, f, g, h, i; for (i = [], d = ["id", "class"], g = a.attributes, e = 0, f = g.length; e < f; e++)b = g[e], h = b.nodeName, c.call(d, h) < 0 && i.push("[" + b.nodeName + "=" + b.nodeValue + "]"); return i }, a.prototype.getNthChildSelector = function (a) { var b, c, d, e, f, g; if (e = a.parentNode, null != e) for (b = 0, g = e.childNodes, c = 0, d = g.length; c < d; c++)if (f = g[c], this.isElement(f) && (b++ , f === a)) return ":nth-child(" + b + ")"; return null }, a.prototype.testSelector = function (a, b) { var c, d; return c = !1, null != b && "" !== b && (d = a.ownerDocument.querySelectorAll(b), 1 === d.length && d[0] === a && (c = !0)), c }, a.prototype.testUniqueness = function (a, b) { var c, d; return d = a.parentNode, c = d.querySelectorAll(b), 1 === c.length && c[0] === a }, a.prototype.testCombinations = function (a, b, c) { var d, e, f, g, h, i, j; for (i = this.getCombinations(b), e = 0, g = i.length; e < g; e++)if (d = i[e], this.testUniqueness(a, d)) return d; if (null != c) for (j = b.map(function (a) { return c + a }), f = 0, h = j.length; f < h; f++)if (d = j[f], this.testUniqueness(a, d)) return d; return null }, a.prototype.getUniqueSelector = function (a) { var b, c, d, e, f, g, h; for (h = this.getTagSelector(a), d = this.options.selectors, b = 0, c = d.length; b < c; b++) { switch (f = d[b]) { case "id": e = this.getIdSelector(a); break; case "tag": h && this.testUniqueness(a, h) && (e = h); break; case "class": g = this.getClassSelectors(a), null != g && 0 !== g.length && (e = this.testCombinations(a, g, h)); break; case "attribute": g = this.getAttributeSelectors(a), null != g && 0 !== g.length && (e = this.testCombinations(a, g, h)); break; case "nthchild": e = this.getNthChildSelector(a) }if (e) return e } return "*" }, a.prototype.getSelector = function (a) { var b, c, d, e, f, g, h; for (h = [], e = this.getParents(a), c = 0, d = e.length; c < d; c++)if (b = e[c], g = this.getUniqueSelector(b), null != g && (h.unshift(g), f = h.join(" > "), this.testSelector(a, f))) return f; return null }, a.prototype.getCombinations = function (a) { var b, c, d, e, f, g, h; for (null == a && (a = []), h = [[]], b = d = 0, f = a.length - 1; 0 <= f ? d <= f : d >= f; b = 0 <= f ? ++d : --d)for (c = e = 0, g = h.length - 1; 0 <= g ? e <= g : e >= g; c = 0 <= g ? ++e : --e)h.push(h[c].concat(a[b])); return h.shift(), h = h.sort(function (a, b) { return a.length - b.length }), h = h.map(function (a) { return a.join("") }) }, a }(), ("undefined" != typeof define && null !== define ? define.amd : void 0) ? define([], function () { return a }) : (b = "undefined" != typeof exports && null !== exports ? exports : this, b.CssSelectorGenerator = a) }).call(this);