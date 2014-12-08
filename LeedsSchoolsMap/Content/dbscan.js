// from https://code.google.com/p/education-data-ma/source/browse/trunk/data+analysis/clustering/dbscan.js

// DBSCAN constructs a DBSCAN cluster analyzer.
// This implements the DBSCAN algorithm published in Wikipedia's DBSCAN article
// as of October 19, 2010. I'm including the algorithm as published, with my notes,
// at the end of this file. I've used the same variable names in the Javascript code
// to help correlate the code and algorithm/article.
//
// Call the DBSCAN constructor with four or five arguments:
//      an array of data points     D
//      a distance function         dist
//      an epsilon value            eps
//      a minimum cluster size      MinPts
//      an initialization function  init   (optional)
//
// You may update these as you wish before running or rerunning an analysis.
// Run your DBSCAN by calling its run() method.
// Run() accepts optional epsilon and cluster size arguments. 
// You may also set those and other properties directly before running a DBSCAN.
//
// If you supply an init function, the constructor will call it as a last step
// in constructing the DBSCAN analyzer. The run() method will also call it before
// each DBSCAN run.
//
// D may be an array of primitive values or any kind of object. Only dist()
// needs to understand the structure of D elements to compute the distance
// between any two of them, given their indices. Other DBSCAN routines merely 
// treat data points as index values.

function DBSCAN (D, dist, eps, MinPts, init) {
    // Construct a DBSCAN cluster analyzer.
    this.D = D;           // array of data points
    this.dist = dist;     // distance function(i1, i2) given indices of two data points
    if (init)
      this.init = init;   // optional initialization function, to run upon construction
                          //   and before each analysis run
    this.eps = eps;       // neighborhood radius
    this.MinPts = MinPts; // minimum number of points to form cluster
    this.assigned = [];   // cluster assignment 
                          //          >= 0 --> cluster index
                          //            -1 --> noise
                          //     undefined --> we have not yet visited the point
    this.cluster = [];    // array of clusters, each an array of point indices
    // Note that we store cluster assignments redundantly in this.assigned and this.cluster
    // to quickly determine which points are in a cluster and which cluster a point is in.
    // Always update both arrays!
    this.run = dbscanRun;  // run the analysis, optionally with new eps, MinPts values
    this.getNeighbors = dbscanGetNeighbors;   // private
    this.expandCluster = dbscanExpandCluster; // private
    if (this.init) this.init();
}

function dbscanRun(eps, MinPts) {
    if (eps) this.eps = eps;
    if (MinPts) this.MinPts = MinPts;
    this.assigned = new Array(this.D.length);
    this.cluster = new Array();
    if (this.init) this.init();
    for (var P in this.D) {
        if (this.assigned[P] === undefined) {  // if we haven't visited this point already
            console.log('visiting ' + P);
            var N = this.getNeighbors(P);
            // console.log(N.length + ' neighbors: ' + N);
            if (N.length + 1 < this.MinPts) {
                this.assigned[P] = -1;  // noise
                // console.log('noise');
            }
            else {
                var C = this.cluster.length;  // next cluster index
                this.cluster[C] = [];         // new cluster
                // console.log('cluster ' + C);
                this.expandCluster(P, N, C);
            }
        }
    }
    // for (var c in this.cluster) {
    //     console.log('cluster ' + c + ' size ' + this.cluster[c].length + ': ' + this.cluster[c]);
    // }
}

function dbscanGetNeighbors(P) {
    var neighbors = [];
    for (var i in this.D) {
        if (i == P) continue;
        if (this.dist(P, i) <= this.eps)
            neighbors.push(i);
    }
    return neighbors;
}

function dbscanExpandCluster(P, N, C) {
    this.cluster[C].push(P);  // add point index to cluster
    this.assigned[P] = C;     // store cluster index in point assignment
    for (var PP = 0; PP < N.length; PP++) {  // PP means P' -- note P' is indexing N, not D
        // console.log('> ' + N[PP]);
        if (this.assigned[N[PP]] === undefined) {  // P' not yet visited?
            // console.log('> visiting ' + N[PP]);
            var NP = this.getNeighbors(N[PP]);     // NP means N'
            // console.log('> ' + NP.length + ' neighbors: ' + NP);
            if (NP.length + 1 >= this.MinPts) {
                N = N.concat(NP.filter(function(p) { return p != P && N.indexOf(p) == -1 } ));
                // console.log('expanded neighborhood: ' + N);
            }
        }
        if (!(this.assigned[N[PP]] > -1)) {  // P' not yet assigned to a cluster?
            this.cluster[C].push(N[PP]);  // add point index to cluster
            this.assigned[N[PP]] = C;     // store cluster index in point assignment
        }
    }
}