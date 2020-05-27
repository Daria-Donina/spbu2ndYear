module Task2

type Tree<'a> =
    | Node of 'a * Tree<'a> * Tree<'a>
    | Empty

// Applies function to the every element of a binary tree.
let treeMap mapping (tree: Tree<'a>) =

    let rec treeMapRec binTree =
        match binTree with
        | Empty -> Empty
        | Node(x, l, r) -> Node(mapping x, treeMapRec l, treeMapRec r)

    treeMapRec tree
