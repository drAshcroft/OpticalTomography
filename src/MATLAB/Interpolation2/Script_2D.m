
%[allSlices, block] = LoadInterpolateImage2D(100,100);

coords =cell([length(allSlices) 1]);
maxes=zeros([length(allSlices) 1]);
for I=1:length(allSlices)
    [m idx1]= max(allSlices{I}.array);
    [m idxY]=max(m);
    
    maxes(I)=m;
    coords{I}=[idxY idx1(idxY)];
    
end

[maxM idx]=max(maxes);

maxM=maxM*.95;

firstCent = coords{idx};

R=20;
cc=1;
for I=1:length(allSlices)
    
    if (maxes(I)>maxM)
        
        c=coords{I};
        r=(( c(1)-firstCent(1) )^2 +( c(2)-firstCent(2) )^2)^.5/R;
        if (r<1)
            candidate(cc)=I;
            cc=cc+1;
        end
        
    end
end

R=50;
secondMax =0;
secondPeaks =cell([length(candidate) 1]);
secLocations = cell(size(secondPeaks));
myfilter = fspecial('gaussian',[19 19], 10);
figure;colormap gray;
for I=1:length(candidate);
    image = allSlices{candidate(I)}.array;
    im2=imfilter(image,myfilter);
    im2=im2-maxM/2;
    im2(im2<0)=0;
    im2=imregionalmax(im2);
    
    [row col] = find (im2>0);
    clf;
    imagesc( image);
    hold all;
    % cent= FastPeakFind(image,maxM/4,myfilter,15);
    
    maxes = zeros([1 size(col,1)]);
    for J=1:length(col)
        maxes(J) = image(col(J),row(J));
    end
    
    r=(( col-firstCent(1) ).^2 +( row-firstCent(2) ).^2).^.5./R;
    idx=find(r>1);
    secondPeaks{I} = maxes(idx);
    secLocations{I} = [col(idx) row(idx)];
    
    [m idx2]=max(maxes);
    if m>secondMax
        
        secondMax = m;
        maxLoc = [col(idx2) row(idx2)];
        secondImageIndex=I;
    end
    
    scatter(col(idx),row(idx));
    drawnow;
end



